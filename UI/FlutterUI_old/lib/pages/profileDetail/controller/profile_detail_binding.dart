import 'package:get/get.dart';
import 'package:mobile/pages/profileDetail/controller/profile_detail_controller.dart';

class ProfileDetailBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<ProfileDetailController>(() => ProfileDetailController());
  }
}
