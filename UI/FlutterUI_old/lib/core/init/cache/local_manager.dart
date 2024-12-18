import 'dart:convert';
import 'dart:io';

import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/login_response_model.dart';
import 'package:path_provider/path_provider.dart';
import 'package:shared_preferences/shared_preferences.dart';

class LocaleManager {
  static Future prefrencesInit() async {
    instance._preferences ??= await SharedPreferences.getInstance();
  }

  static final LocaleManager _instance = LocaleManager._init();

  static LocaleManager get instance => _instance;

  SharedPreferences? _preferences;

  LocaleManager._init() {
    SharedPreferences.getInstance().then((value) {
      _preferences = value;
    });
  }

  Future<void> clearAllSaveFirst() async {
    if (_preferences != null) {
      await _preferences!.clear();
      NetworkManager.instance.dio.options.headers.remove("Authorization");
    }
  }

  Future<void> clearCache() async {
    if (_preferences != null) {
      await _preferences!.remove(PreferencesKeys.TOKEN.toString());
      await _preferences!.remove(PreferencesKeys.TOKENEXPIRATION.toString());
      await _preferences!.remove(PreferencesKeys.USER.toString());
      await _preferences!.remove(PreferencesKeys.LOGINMODEL.toString());
      NetworkManager.instance.dio.options.headers.remove("Authorization");
    }
  }

  Future<void> setStringValue(PreferencesKeys key, String value) async {
    await _preferences!.setString(key.toString(), value);
  }

  Future<void> setUserModelValue(
      PreferencesKeys key, LoginResponseModel userModel) async {
    final SharedPreferences prefs = await SharedPreferences.getInstance();
    final jsonString = jsonEncode(userModel.toJson());
    await prefs.setString(key.toString(), jsonString);
  }

  Future<void> setUser(PreferencesKeys key, UserInfo userModel) async {
    final SharedPreferences prefs = await SharedPreferences.getInstance();
    final jsonString = jsonEncode(userModel.toJson());
    await prefs.setString(key.toString(), jsonString);
  }

  Future<void> setStringValue2(String key, String value) async {
    await _preferences!.setString(key, value);
  }

  Future<void> setBoolValue(PreferencesKeys key, bool value) async {
    await _preferences!.setBool(key.toString(), value);
  }

  String getStringValue(PreferencesKeys key) =>
      _preferences?.getString(key.toString()) ?? '';
  String getStringValue2(String key) => _preferences?.getString(key) ?? '';

  Future<LoginResponseModel?> getUserModelValue(PreferencesKeys key) async {
    final SharedPreferences prefs = await SharedPreferences.getInstance();
    final jsonString = prefs.getString(key.toString());
    if (jsonString != null) {
      final jsonMap = jsonDecode(jsonString) as Map<String, dynamic>;
      return LoginResponseModel.fromJson(jsonMap);
    }
    return null;
  }

  Future<UserInfo?> getUser(PreferencesKeys key) async {
    final SharedPreferences prefs = await SharedPreferences.getInstance();
    final jsonString = prefs.getString(key.toString());
    if (jsonString != null) {
      final jsonMap = jsonDecode(jsonString) as Map<String, dynamic>;
      return UserInfo.fromJson(jsonMap);
    }
    return null;
  }

  bool getBoolValue(PreferencesKeys key) =>
      _preferences!.getBool(key.toString()) ?? false;

  Future<void> saveImageToLocal(File imageFile) async {
    List<int> imageBytes = await imageFile.readAsBytes();
    String base64Image = base64Encode(imageBytes);
    SharedPreferences preferences = await SharedPreferences.getInstance();
    await preferences.setString('profile_image', base64Image);
  }

  Future<File?> getImageFromLocal() async {
    SharedPreferences preferences = await SharedPreferences.getInstance();
    String? base64Image = preferences.getString('profile_image');
    if (base64Image != null) {
      List<int> imageBytes = base64Decode(base64Image);
      final directory = await getTemporaryDirectory();
      final imageFile = File('${directory.path}/profile_image.png');
      await imageFile.writeAsBytes(imageBytes);
      return imageFile;
    }
    return null;
  }
}
