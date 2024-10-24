import 'dart:io';

import 'package:dio/dio.dart';
import 'package:mobile/core/base/base_service.dart';
import 'package:mobile/core/enums/service_endpoints.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/scan/model/add_question_response_model.dart';
import 'package:mobile/pages/scan/model/get_question_model.dart';
import 'package:mobile/pages/scan/model/lesson_response_model.dart';
import 'package:mobile/pages/scan/model/similar_question_response_model.dart';
import 'package:mobile/pages/similarQuestions/model/get_similar_question_response_model.dart';

class ScanService extends BaseService {
  ScanService(super.dio);

  Future scanQuestion(Map<String, dynamic> json) async {
    getAuth();
    try {
      final response =
          await dio.post(ServiceEndpoints.addQuestion.getEndpoint, data: json);
      if (response.statusCode == HttpStatus.ok) {
        final data = AddQuestionResponseModel.fromJson(response.data);
        return data;
      }
      if (response.statusCode == 400) {
        return ErrorModel(statusCode: 400, message: response.data['message']);
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;
      return ErrorModel(statusCode: 400, message: errorJson.data['message']);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }

  Future scanSimilarQuestion(Map<String, dynamic> json) async {
    getAuth();
    try {
      final response = await dio
          .post(ServiceEndpoints.addSimilarQuestion.getEndpoint, data: json);
      if (response.statusCode == HttpStatus.ok) {
        final data = SimilarQuestionResponseModel.fromJson(response.data);
        return data;
      }
      if (response.statusCode == 400) {
        return ErrorModel(statusCode: 400, message: response.data['message']);
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;
      return ErrorModel(statusCode: 400, message: errorJson['message']);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }

  Future getQuestions(Map<String, dynamic> json) async {
    getAuth();
    try {
      final response = await dio.post(
          '${ServiceEndpoints.getQuestions.getEndpoint}/?Page=0&PageSize=0',
          data: json);
      if (response.statusCode == HttpStatus.ok) {
        final data = GetQuestionsResponseModel.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;
      return ErrorModel.fromJson(errorJson);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }

  Future getSimilarQuestions(Map<String, dynamic> json) async {
    getAuth();
    try {
      final response = await dio.post(
          '${ServiceEndpoints.getSimilarQuestion.getEndpoint}/?Page=0&PageSize=0',
          data: json);
      if (response.statusCode == HttpStatus.ok) {
        final data = GetSimilarQuestionResponseModel.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;
      return ErrorModel.fromJson(errorJson);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }

  Future getLessons() async {
    getAuth();
    try {
      final response = await dio.get(
        '${ServiceEndpoints.getLessons.getEndpoint}/?Page=0&PageSize=0',
      );
      if (response.statusCode == HttpStatus.ok) {
        final data = LessonResponseModel.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;
      return ErrorModel.fromJson(errorJson);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }

  Future readQuestion(String questionId) async {
    getAuth();
    try {
      final response = await dio.post(
          ServiceEndpoints.isReadQuestion.getEndpoint,
          queryParameters: {'questionId': questionId});
      if (response.statusCode == HttpStatus.ok) {
        return true;
      }
    } catch (e) {
      return false;
    }
    return false;
  }
}
