// To parse this JSON data, do
//
//     final loginPostModel = loginPostModelFromJson(jsonString);

import 'dart:convert';

LoginPostModel loginPostModelFromJson(String str) =>
    LoginPostModel.fromJson(json.decode(str));

String loginPostModelToJson(LoginPostModel data) => json.encode(data.toJson());

class LoginPostModel {
  String userName;
  String password;
  int authenticatorCode;

  LoginPostModel({
    required this.userName,
    required this.password,
    required this.authenticatorCode,
  });

  factory LoginPostModel.fromJson(Map<String, dynamic> json) => LoginPostModel(
        userName: json["userName"],
        password: json["password"],
        authenticatorCode: json["authenticatorCode"],
      );

  Map<String, dynamic> toJson() => {
        "userName": userName,
        "password": password,
        "authenticatorCode": authenticatorCode,
      };
}
