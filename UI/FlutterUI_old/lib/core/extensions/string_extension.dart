import 'dart:ui';

import 'package:mobile/core/constants/string_constant.dart';

class StringExtension {
  static bool isEmail(String em) {
    String p = StringConstants.emailRegex;
    RegExp regExp = RegExp(p);
    return regExp.hasMatch(em);
  }

  static String formatPhoneNumber(String phoneNumber) {
    String cleanedNumber =
        phoneNumber.replaceAll(RegExp(StringConstants.onlyNumberRegex), '');
    return cleanedNumber;
  }


}
