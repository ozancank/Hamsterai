import 'package:get/get.dart';
import 'scan_controller.dart';

class ScanBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<ScanController>(() => ScanController());
  }
}
