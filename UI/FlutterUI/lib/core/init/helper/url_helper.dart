import 'package:mobile/core/constants/app_constant.dart';

class UrlHelper {
  static String getDynamicBaseUrl(String? email) {
    if (email == null) {
      return ApplicationConstants.APIBASEURL;
    }
    return ApplicationConstants.getBaseUrl(email);
  }
}
