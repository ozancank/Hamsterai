import 'dart:io';
import 'package:dio/dio.dart';
import 'package:mobile/core/base/base_service.dart';
import 'package:mobile/core/enums/locale_keys_enum.dart';
import 'package:mobile/core/enums/service_endpoints.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/auth/model/login_response_model.dart';

class AuthService extends BaseService {
  AuthService(super.dio);

  Future login(Map<String, dynamic> json) async {
    getAuth();
    try {
      final response =
          await dio.post(ServiceEndpoints.login.getEndpoint, data: json);
      if (response.statusCode == HttpStatus.ok) {
        final data = LoginResponseModel.fromJson(response.data);
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

  Future<bool> logout() async {
    getAuth();
    try {
      final response = await dio.get(ServiceEndpoints.logout.getEndpoint);
      if (response.statusCode == HttpStatus.ok) {
        return true;
      }
    } on DioException {
      return false;
    } catch (e) {
      return false;
    }
    return false;
  }

  Future getUserById(int userId) async {
    getAuth();
    try {
      final response =
          await dio.get('${ServiceEndpoints.getUserById.getEndpoint}/$userId');
      if (response.statusCode == HttpStatus.ok) {
        final data = UserInfo.fromJson(response.data);
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


  Future forgotPassword(String email) async {
    getAuth();
    try {
      final response = await dio.post(
          ServiceEndpoints.forgotPassword.getEndpoint,
          data: {"email": email});
      if (response.statusCode == HttpStatus.ok) {
        return true;
      }
    } on DioException catch (e) {
      print(e);
      return false;
    } catch (e) {
      return false;
    }

    return false;
  }

  Future addDeviceToken() async {
    getAuth();
    try {
      var token =
          LocaleManager.instance.getStringValue(PreferencesKeys.FIREBASETOKEN);
      final response = await dio.post(
          ServiceEndpoints.addDeviceToken.getEndpoint,
          data: {"deviceToken": token});
      if (response.statusCode == HttpStatus.ok) {
        return true;
      }
    } on DioException catch (e) {
      print(e);
      return false;
    } catch (e) {
      return false;
    }

    return false;
  }
}
