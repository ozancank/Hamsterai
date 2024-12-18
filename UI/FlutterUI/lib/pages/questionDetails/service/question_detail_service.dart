import 'dart:io';

import 'package:mobile/core/base/base_service.dart';
import 'package:mobile/core/enums/service_endpoints.dart';

class QuestionDetailService extends BaseService {
  QuestionDetailService(super.dio);

  Future reSendQuestion(String questionId) async {
    getAuth();
    try {
      final response = await dio.post(
          ServiceEndpoints.manuelSendAgain.getEndpoint,
          queryParameters: {'questionId': questionId});
      if (response.statusCode == HttpStatus.ok) {
        return true;
      }
    } catch (e) {
      return false;
    }
    return false;
  }

  Future updateQuestionText(String questionId, String questionText) async {
    getAuth();
    try {
      final response = await dio.post(
          ServiceEndpoints.updateQuestionText.getEndpoint,
          data: {'questionId': questionId, 'questionText': questionText});
      if (response.statusCode == HttpStatus.ok) {
        return true;
      }
    } catch (e) {
      return false;
    }
    return false;
  }
}
