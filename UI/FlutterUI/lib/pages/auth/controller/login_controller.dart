import 'dart:io';

import 'package:android_id/android_id.dart';
import 'package:device_info_plus/device_info_plus.dart';
import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/core/base/base_controller.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/core/routers/pages.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/auth/model/login_post_model.dart';
import 'package:mobile/pages/auth/model/login_response_model.dart';
import 'package:mobile/pages/auth/service/auth_service.dart';
import 'package:mobile/styles/colors.dart';
import 'package:quickalert/quickalert.dart';

class LoginController extends BaseController {
  var emailIconColor = const Color(0xFFADB4C0);
  var emailIconColorForgot = const Color(0xFFADB4C0);
  var passwordIconColor = const Color(0xFFADB4C0);
  var labelTextColor = const Color(0xFF000000);
  var buttonColor = const Color(0xFFFFFFFF);
  var textColor = const Color(0xFF838FA0);
  var buttonColorForgot = const Color(0xFFFFFFFF);
  var textColorForgot = const Color(0xFF838FA0);
  var labelTextColorForgot = const Color(0xFF000000);

  late AuthService _authService;
  int? userId;
  String? deviceId;
  LoginResponseModel? userResponseModel;
  @override
  void onInit() {
    _authService = AuthService(NetworkManager.instance.dio);
    super.onInit();
  }

  Future<bool> login(LoginPostModel model, BuildContext context) async {
    EasyLoading.show(
        status: 'Giriş Yapılıyor', maskType: EasyLoadingMaskType.black);

    String baseUrl = ApplicationConstants.getBaseUrl(model.userName);
    NetworkManager.instance.setBaseUrl(baseUrl);
    final Map<String, dynamic> tempJson = model.toJson();
    var loginModel = await _authService
        .login(tempJson);
    if (loginModel == null) {
      EasyLoading.showError('Giriş yapılamadı.');
      return false;
    }

    if (loginModel is LoginResponseModel) {
      EasyLoading.dismiss();
      await setCacheData(
          loginModel.accessToken.token,
          loginModel.accessToken.expiration.toIso8601String(),
          loginModel,
          loginModel.userInfo.mustPasswordChange!);
      _authService.addDeviceToken();
      userResponseModel = loginModel;

      if (userResponseModel!.userInfo.mustPasswordChange == true) {
        Get.offAllNamed(Routes.FORCEUPDATEPASSWORD);
        return true;
      } else {
        await LocaleManager.instance
            .setBoolValue(PreferencesKeys.ISMUSTPASSWORDCHANGE, false);
        Get.offAllNamed('/home');
        return true;
      }
    } else if (loginModel is ErrorModel) {
      EasyLoading.dismiss();
      QuickAlert.show(
        context: context,
        type: QuickAlertType.error,
        text: loginModel.message,
        confirmBtnText: 'Tamam',
        confirmBtnColor: MyColors.redColor,
        confirmBtnTextStyle: Theme.of(context)
            .textTheme
            .bodyMedium!
            .copyWith(color: Colors.white),
        title: 'Hata',
      );
      return false;
    }
    return false;
  }

  Future<void> logout(context) async {
    EasyLoading.show(status: 'Yükleniyor', maskType: EasyLoadingMaskType.black);
    var logout = await _authService.logout();
    if (logout) {
      EasyLoading.dismiss();
      Get.offAllNamed('/login');
      LocaleManager.instance.clearCache();
    } else {
      EasyLoading.dismiss();
      return QuickAlert.show(
        context: context,
        type: QuickAlertType.error,
        text: 'Çıkış yaparken bir hata oluştu.',
        confirmBtnText: 'Tamam',
        confirmBtnColor: MyColors.redColor,
        confirmBtnTextStyle: Theme.of(context)
            .textTheme
            .bodyMedium!
            .copyWith(color: Colors.white),
        title: 'Hata',
      );
    }
  }

  Future<bool> forgotPassword(context, String email) async {
    EasyLoading.show(status: 'Yükleniyor', maskType: EasyLoadingMaskType.black);
    var forgotPassword = await _authService.forgotPassword(email);
    if (forgotPassword) {
      EasyLoading.dismiss();
      Get.offAllNamed('/login');
      return true;
    } else {
      EasyLoading.dismiss();
      QuickAlert.show(
        context: context,
        type: QuickAlertType.error,
        text: 'Kullanıcı kaydı bulunamadı',
        confirmBtnText: 'Tamam',
        confirmBtnColor: MyColors.redColor,
        confirmBtnTextStyle: Theme.of(context)
            .textTheme
            .bodyMedium!
            .copyWith(color: Colors.white),
        title: 'Hata',
      );
      return false;
    }
  }

  Future<void> getUserById(context) async {
    EasyLoading.show(status: 'Yükleniyor', maskType: EasyLoadingMaskType.black);
    var getUserById = await _authService.getUserById(userModel!.id);
    if (getUserById is UserInfo) {
      EasyLoading.dismiss();
      Get.toNamed('/profile');
    } else if (getUserById is ErrorModel) {
      EasyLoading.dismiss();
      return QuickAlert.show(
        context: context,
        type: QuickAlertType.error,
        text: getUserById.message,
        confirmBtnText: 'Tamam',
        confirmBtnColor: MyColors.redColor,
        confirmBtnTextStyle: Theme.of(context)
            .textTheme
            .bodyMedium!
            .copyWith(color: Colors.white),
        title: 'Hata',
      );
    }
  }

  // Future<void> addDeviceToken() async {
  //   getId().then((value) {
  //     if (value != null) {
  //       _authService.addDeviceToken(value);
  //     }
  //   });
  // }

  Future<String?> getId() async {
    try {
      if (Platform.isIOS) {
        final deviceInfo = DeviceInfoPlugin();
        final iosDeviceInfo = await deviceInfo.iosInfo;
        deviceId = iosDeviceInfo.identifierForVendor ?? '';
        return deviceId;
      } else {
        const androidIdPlugin = AndroidId();
        deviceId = await androidIdPlugin.getId() ?? '';
        return deviceId;
      }
    } catch (_) {
      print("$_ device id error");
      return null;
    }
  }
}
