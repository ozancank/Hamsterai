import 'package:get/get.dart';

import 'statistics_controller.dart';

class StatisticsBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<StatisticsController>(() => StatisticsController());
  }
}
