import 'dart:async';

import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:get/get.dart';
import 'package:mobile/core/base/initial_bindings.dart';
import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/core/routers/pages.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:mobile/module/no_connection_view.dart';
import 'package:mobile/pages/auth/service/auth_service.dart';
import 'package:mobile/pages/home/controller/home_page_controller.dart';
import 'firebase_options.dart';

@pragma('vm:entry-point')
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  // If you're going to use other Firebase services in the background, such as Firestore,
  // make sure you call `initializeApp` before using other Firebase services.
  //await Firebase.initializeApp();

  print("Handling a background message: ${message.messageId}");
}

const AndroidNotificationChannel channel = AndroidNotificationChannel(
  'high_importance_channel', // id
  'High Importance Notifications', // name
  importance: Importance.high,
);

final FlutterLocalNotificationsPlugin flutterLocalNotificationsPlugin =
    FlutterLocalNotificationsPlugin();

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );
  requestNotificationPermission();
  await FirebaseMessaging.instance.setAutoInitEnabled(true);
  await LocaleManager.prefrencesInit();
  SystemChrome.setPreferredOrientations([
    DeviceOrientation.portraitUp,
  ]);
  FirebaseMessaging.onBackgroundMessage(_firebaseMessagingBackgroundHandler);
  FirebaseMessaging.onMessage.listen((RemoteMessage message) {
    if (message.notification != null) {
      print('Message also contained a notification: ${message.notification}');
    }
  });
  await flutterLocalNotificationsPlugin
      .resolvePlatformSpecificImplementation<
          AndroidFlutterLocalNotificationsPlugin>()
      ?.createNotificationChannel(channel);

  final fCMToken = await FirebaseMessaging.instance.getToken();
  await LocaleManager.instance
      .setStringValue(PreferencesKeys.FIREBASETOKEN, fCMToken ?? "Empty");
  if (kDebugMode) {
    print('FCM TOKEN: $fCMToken');
  }
  final connectivityResult = await (Connectivity().checkConnectivity());
  if (connectivityResult[0].name == "none") {
    runApp(
      const MaterialApp(
        debugShowCheckedModeBanner: false,
        home: NoConnection(),
      ),
    );
  } else {
    runApp(const MyApp());
  }
}

Future<void> requestNotificationPermission() async {
  FirebaseMessaging messaging = FirebaseMessaging.instance;

  NotificationSettings settings = await messaging.requestPermission(
    alert: true,
    announcement: true,
    badge: true,
    carPlay: false,
    criticalAlert: false,
    provisional: false,
    sound: true,
  );

  print('User granted permission: ${settings.authorizationStatus}');
}

class MyApp extends StatefulWidget {
  const MyApp({super.key});

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  final HomePageController homePageController = Get.put(HomePageController());
  late AuthService _authService;
  @override
  void initState() {
    super.initState();

    _authService = AuthService(NetworkManager.instance.dio);
    var initializationSettingsAndroid =
        const AndroidInitializationSettings('@mipmap/ic_launcher');
    var initializationSettings =
        InitializationSettings(android: initializationSettingsAndroid);

    flutterLocalNotificationsPlugin.initialize(initializationSettings);

    FirebaseMessaging.onMessage.listen((RemoteMessage message) {
      RemoteNotification? notification = message.notification;
      AndroidNotification? android = message.notification?.android;

      if (notification != null && android != null) {
        homePageController
            .addNotification(notification.body ?? "Yeni bildirim");
        flutterLocalNotificationsPlugin.show(
          notification.hashCode,
          notification.title,
          notification.body,
          NotificationDetails(
            android: AndroidNotificationDetails(
              channel.id,
              channel.name,
              icon: '@mipmap/ic_launcher',
            ),
          ),
        );
      }
    });
    FirebaseMessaging.instance.onTokenRefresh.listen((newToken) async {
      print("FCM Token yenilendi: $newToken");
      await LocaleManager.instance
          .setStringValue(PreferencesKeys.FIREBASETOKEN, newToken);
      _authService.addDeviceToken();
    });
  }

  @override
  Widget build(BuildContext context) {
    return GetMaterialApp(
      builder: EasyLoading.init(),
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
        fontFamily: 'Nunito',
        textTheme: const TextTheme(
          bodyLarge: TextStyle(fontSize: 16.0),
          bodyMedium: TextStyle(fontSize: 14.0),
          displayLarge: TextStyle(fontSize: 96.0),
          displayMedium: TextStyle(fontSize: 60.0),
          displaySmall: TextStyle(fontSize: 48.0),
          headlineMedium: TextStyle(fontSize: 34.0),
          headlineSmall: TextStyle(fontSize: 24.0),
          titleLarge: TextStyle(fontSize: 20.0),
          titleMedium: TextStyle(fontSize: 16.0),
          titleSmall: TextStyle(fontSize: 14.0),
          bodySmall: TextStyle(fontSize: 12.0),
          labelLarge: TextStyle(fontSize: 14.0),
          labelSmall: TextStyle(fontSize: 10.0),
        ),
      ),
      getPages: AppPages.pages,
      initialRoute: router(),
      initialBinding: InitialBinding(),
    );
  }

  String router() {
    var token = LocaleManager.instance.getStringValue(PreferencesKeys.TOKEN);
    var expiration =
        LocaleManager.instance.getStringValue(PreferencesKeys.TOKENEXPIRATION);
    if (token.isNotEmpty) {
      DateTime expirationDate = DateTime.parse(expiration);
      if (expirationDate.isBefore(DateTime.now())) {
        return Routes.LOGIN;
      }
      return Routes.HOME;
    } else {
      return Routes.LOGIN;
    }
  }
}
