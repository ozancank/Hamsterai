import 'package:get/get.dart';
import 'package:mobile/pages/settings/controller/settings_controller.dart';

class SettingsBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<SettingsController>(() => SettingsController());
  }
}
