import 'package:intl/intl.dart';

extension DateTimeStringFormatting on String {
  String toFormattedDateTime() {
    try {
      DateTime dateTime = DateTime.parse(this);
      DateFormat dateFormat = DateFormat('yyyy-MM-dd HH:mm');
      return dateFormat.format(dateTime);
    } catch (e) {
      return '';
    }
  }

  String toFormattedDateTime2() {
    try {
      DateTime dateTime = DateTime.parse(this);
      DateFormat dateFormat = DateFormat('yyyy-MM-dd');
      return dateFormat.format(dateTime);
    } catch (e) {
      return '';
    }
  }
}
