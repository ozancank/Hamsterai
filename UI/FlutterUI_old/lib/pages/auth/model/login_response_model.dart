import 'dart:convert';

LoginResponseModel loginResponseModelFromJson(String str) =>
    LoginResponseModel.fromJson(json.decode(str));

String loginResponseModelToJson(LoginResponseModel data) =>
    json.encode(data.toJson());

class LoginResponseModel {
  Token accessToken;
  Token refreshToken;
  UserInfo userInfo;

  LoginResponseModel({
    required this.accessToken,
    required this.refreshToken,
    required this.userInfo,
  });

  factory LoginResponseModel.fromJson(Map<String, dynamic> json) =>
      LoginResponseModel(
        accessToken: Token.fromJson(json["accessToken"]),
        refreshToken: Token.fromJson(json["refreshToken"]),
        userInfo: UserInfo.fromJson(json["userInfo"]),
      );

  Map<String, dynamic> toJson() => {
        "accessToken": accessToken.toJson(),
        "refreshToken": refreshToken.toJson(),
        "userInfo": userInfo.toJson(),
      };
}

class Token {
  String token;
  DateTime expiration;

  Token({
    required this.token,
    required this.expiration,
  });

  factory Token.fromJson(Map<String, dynamic> json) => Token(
        token: json["token"],
        expiration: DateTime.parse(json["expiration"]),
      );

  Map<String, dynamic> toJson() => {
        "token": token,
        "expiration": expiration.toIso8601String(),
      };
}

class UserInfo {
  int id;
  bool? isActive;
  String? userName;
  String? name;
  String? surname;
  String? phone;
  String? profileFileName;
  String? email;
  String? fullName;
  int? connectionId;
  int? schoolId;
  int? type;

  UserInfo({
    required this.id,
    this.isActive,
    this.userName,
    this.name,
    this.surname,
    this.phone,
    this.profileFileName,
    this.email,
    this.fullName,
    this.connectionId,
    this.schoolId,
    this.type,
  });

  factory UserInfo.fromJson(Map<String, dynamic> json) => UserInfo(
        id: json["id"],
        isActive: json["isActive"],
        userName: json["userName"],
        name: json["name"],
        surname: json["surname"],
        phone: json["phone"],
        profileFileName: json["profileFileName"],
        email: json["email"],
        fullName: json["fullName"],
        connectionId: json["connectionId"],
        schoolId: json["schoolId"],
        type: json["type"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "isActive": isActive,
        "userName": userName,
        "name": name,
        "surname": surname,
        "phone": phone,
        "profileFileName": profileFileName,
        "email": email,
        "fullName": fullName,
        "connectionId": connectionId,
        "schoolId": schoolId,
        "type": type,
      };
}
