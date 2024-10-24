import 'package:flutter_tts/flutter_tts.dart';
import 'package:mobile/core/base/base_controller.dart';

class TTSController extends BaseController {
  final FlutterTts flutterTts = FlutterTts();

  Future<void> speak(String text) async {
    List<String> parts = _splitTextByLanguage(text);
    for (String part in parts) {
      if (_isEnglishText(part)) {
        await flutterTts.setLanguage("en-US");
      } else {
        await flutterTts.setLanguage("tr-TR");
      }
      await flutterTts.speak(part);
    }
  }

  List<String> _splitTextByLanguage(String text) {
    return text.split(RegExp(r'(?<=\.)|(?<=:)|(?<=\n)'));
  }

  bool _isEnglishText(String text) {
    return RegExp(r'[a-zA-Z]').hasMatch(text);
  }
}
