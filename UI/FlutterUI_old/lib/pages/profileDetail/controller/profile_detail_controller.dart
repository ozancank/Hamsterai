import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/core/base/base_controller.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/auth/model/login_response_model.dart';
import 'package:mobile/pages/auth/model/student_response_model.dart';
import 'package:mobile/pages/profileDetail/model/update_user_post_model.dart';
import 'package:mobile/pages/profileDetail/service/update_user_service.dart';
import 'package:mobile/styles/colors.dart';
import 'package:quickalert/models/quickalert_type.dart';
import 'package:quickalert/widgets/quickalert_dialog.dart';

class ProfileDetailController extends BaseController {
  late UpdateUserService _updateUserService;
  StudentResponseModel? student;

  @override
  void onInit() {
    _updateUserService = UpdateUserService(NetworkManager.instance.dio);
    super.onInit();
  }

  Future<void> updateUser(UpdateUserPostModel model, context) async {
    EasyLoading.show(status: 'Yükleniyor', maskType: EasyLoadingMaskType.black);
    var updateUser = await _updateUserService.updateUser(model);
    if (updateUser is UserInfo) {
      EasyLoading.dismiss();
      updateUserData(updateUser);
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

  Future<void> getStudentById(int userId) async {
    EasyLoading.show(status: 'Yükleniyor', maskType: EasyLoadingMaskType.black);
    var getStudentById = await _updateUserService.getStudentById(userId);
    if (getStudentById is StudentResponseModel) {
      student = getStudentById;
      EasyLoading.dismiss();
      Get.toNamed('/profile');
    } else if (getStudentById is ErrorModel) {
      EasyLoading.dismiss();
    }
  }
}
