import 'package:get/get.dart';
import 'package:mobile/pages/updatePassword/controller/update_password_controller.dart';

class UpdatePasswordBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<UpdatePasswordController>(() => UpdatePasswordController());
  }
}
