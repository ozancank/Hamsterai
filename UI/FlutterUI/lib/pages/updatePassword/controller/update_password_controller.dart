import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/core/base/base_controller.dart';
import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/updatePassword/service/update_password_service.dart';
import 'package:mobile/styles/colors.dart';
import 'package:quickalert/models/quickalert_type.dart';
import 'package:quickalert/widgets/quickalert_dialog.dart';

class UpdatePasswordController extends BaseController {
  late UpdatePasswordService _passwordService;

  @override
  void onInit() {
    _passwordService = UpdatePasswordService(NetworkManager.instance.dio);
    getUserModelFromCache();
    super.onInit();
  }

  Future<void> updatePassword(String oldPassword, String newPassword, context,
      {bool isForce = false}) async {
    EasyLoading.show(
        status: 'Güncelleniyor', maskType: EasyLoadingMaskType.black);
    final tempJson = {
      'id': userModel!.id,
      'oldPassword': oldPassword,
      'password': newPassword
    };
    var updateUser = await _passwordService.updatePassword(tempJson);
    if (updateUser == true) {
      EasyLoading.dismiss();
      await LocaleManager.instance
          .setBoolValue(PreferencesKeys.ISMUSTPASSWORDCHANGE, false);
      // updateUserData(updateUser);
      isForce ? Get.toNamed('/home') : Get.back();
    } else if (updateUser is ErrorModel) {
      EasyLoading.dismiss();
      return QuickAlert.show(
        context: context,
        type: QuickAlertType.error,
        text: updateUser.message,
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
}
