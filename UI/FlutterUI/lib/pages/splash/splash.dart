import 'package:animated_splash_screen/animated_splash_screen.dart';
import 'package:flutter/material.dart';
import 'package:lottie/lottie.dart';
import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/pages/auth/view/login_view.dart';
import 'package:mobile/pages/auth/view/update_password_force_view.dart';
import 'package:mobile/pages/home/view/home_page.dart';
import 'package:mobile/styles/colors.dart';

class SplashPage extends StatelessWidget {
  const SplashPage({super.key});

  @override
  Widget build(BuildContext context) {
    return AnimatedSplashScreen(
      splash: Center(
        child: Column(
          children: [
            Expanded(
                flex: 3, child: LottieBuilder.asset("assets/json/robot.json")),
            Expanded(child: Image.asset("assets/images/alt-yazi.png")),
          ],
        ),
      ),
      nextScreen: router(),
      splashIconSize: 200,
      backgroundColor: MyColors.primaryColor,
      duration: 3000,
    );
  }

  router() {
    var token = LocaleManager.instance.getStringValue(PreferencesKeys.TOKEN);
    var isMustPasswordChange = LocaleManager.instance
        .getBoolValue(PreferencesKeys.ISMUSTPASSWORDCHANGE);

    var expiration =
        LocaleManager.instance.getStringValue(PreferencesKeys.TOKENEXPIRATION);
    if (token.isNotEmpty) {
      if (isMustPasswordChange) {
        return const UpdatePasswordForceView();
      }
      DateTime expirationDate = DateTime.parse(expiration);
      if (expirationDate.isBefore(DateTime.now())) {
        return const LoginView();
      }
      return HomePage();
    } else {
      return const LoginView();
    }
  }
}
