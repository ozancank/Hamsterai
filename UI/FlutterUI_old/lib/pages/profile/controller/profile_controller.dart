import 'package:mobile/core/base/base_controller.dart';

class ProfileController extends BaseController {
  @override
  void onInit() async {
    await getUserModelFromCache();
    super.onInit();
  }
}
