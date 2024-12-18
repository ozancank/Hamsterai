import 'package:get/get.dart';
import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/pages/auth/model/login_response_model.dart';

class BaseController extends GetxController {
  LoginResponseModel? loginModel;
  UserInfo? userModel;
  Future<void> setCacheData(String token, String expiration,
      LoginResponseModel userModel, bool isMustPasswordChange) async {
    await LocaleManager.instance.setStringValue(PreferencesKeys.TOKEN, token);
    await LocaleManager.instance
        .setStringValue(PreferencesKeys.TOKENEXPIRATION, expiration);
    await LocaleManager.instance.setBoolValue(
        PreferencesKeys.ISMUSTPASSWORDCHANGE, isMustPasswordChange);
    await LocaleManager.instance
        .setUserModelValue(PreferencesKeys.LOGINMODEL, userModel);
    await LocaleManager.instance
        .setUser(PreferencesKeys.USER, userModel.userInfo);
  }

  Future<void> updateUserData(UserInfo userModel) async {
    await LocaleManager.instance.setUser(PreferencesKeys.USER, userModel);
  }

  Future<void> setUserProfile(String photoPath) async {
    await LocaleManager.instance
        .setStringValue(PreferencesKeys.USERPHOTO, photoPath);
  }

  getUserModelFromCache() async {
    loginModel = await LocaleManager.instance
        .getUserModelValue(PreferencesKeys.LOGINMODEL);
    userModel = await LocaleManager.instance.getUser(PreferencesKeys.USER);
  }
}
