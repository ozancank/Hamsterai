import 'package:get/route_manager.dart';
import 'package:mobile/core/base/base_controller.dart';
import 'package:mobile/pages/home/view/home_page.dart';

class SplashController extends BaseController {
  @override
  void onReady() {
    navigateToHome();
    super.onReady();
  }

  Future navigateToHome() async {
    await Future.delayed(const Duration(seconds: 3));
    Get.off(() => HomePage());
  }
}
