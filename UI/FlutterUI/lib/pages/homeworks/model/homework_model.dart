import 'dart:convert';

HomeWorkModel homeWorkModelFromJson(String str) =>
    HomeWorkModel.fromJson(json.decode(str));

String homeWorkModelToJson(HomeWorkModel data) => json.encode(data.toJson());

class HomeWorkModel {
  List<HomeWorkItem> items;
  int index;
  int size;
  int count;
  int pages;
  bool hasPrevious;
  bool hasNext;

  HomeWorkModel({
    required this.items,
    required this.index,
    required this.size,
    required this.count,
    required this.pages,
    required this.hasPrevious,
    required this.hasNext,
  });

  factory HomeWorkModel.fromJson(Map<String, dynamic> json) => HomeWorkModel(
        items: List<HomeWorkItem>.from(
            json["items"].map((x) => HomeWorkItem.fromJson(x))),
        index: json["index"],
        size: json["size"],
        count: json["count"],
        pages: json["pages"],
        hasPrevious: json["hasPrevious"],
        hasNext: json["hasNext"],
      );

  Map<String, dynamic> toJson() => {
        "items": List<dynamic>.from(items.map((x) => x.toJson())),
        "index": index,
        "size": size,
        "count": count,
        "pages": pages,
        "hasPrevious": hasPrevious,
        "hasNext": hasNext,
      };
}

class HomeWorkItem {
  String id;
  bool isActive;
  int createUser;
  DateTime createDate;
  int updateUser;
  DateTime updateDate;
  String? teacherName;
  int? studentId;
  String? studentName;
  String? homeworkId;
  String? answerPath;
  int? status;
  Homework? homework;

  HomeWorkItem({
    required this.id,
    required this.isActive,
    required this.createUser,
    required this.createDate,
    required this.updateUser,
    required this.updateDate,
    this.teacherName,
    this.studentId,
    this.studentName,
    this.homeworkId,
    this.answerPath,
    this.status,
    this.homework,
  });

  factory HomeWorkItem.fromJson(Map<String, dynamic> json) => HomeWorkItem(
        id: json["id"],
        isActive: json["isActive"],
        createUser: json["createUser"],
        createDate: DateTime.parse(json["createDate"]),
        updateUser: json["updateUser"],
        updateDate: DateTime.parse(json["updateDate"]),
        teacherName: json["teacherName"],
        studentId: json["studentId"],
        studentName: json["studentName"],
        homeworkId: json["homeworkId"],
        answerPath: json["answerPath"],
        status: json["status"],
        homework: Homework.fromJson(json["homework"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "isActive": isActive,
        "createUser": createUser,
        "createDate": createDate.toIso8601String(),
        "updateUser": updateUser,
        "updateDate": updateDate.toIso8601String(),
        "teacherName": teacherName,
        "studentId": studentId,
        "studentName": studentName,
        "homeworkId": homeworkId,
        "answerPath": answerPath,
        "status": status,
        "homework": homework?.toJson(),
      };
}

class Homework {
  String id;
  bool isActive;
  int createUser;
  DateTime createDate;
  int updateUser;
  DateTime updateDate;
  int? schoolId;
  String? schoolName;
  int? teacherId;
  String? teacherName;
  int? lessonId;
  String? lessonName;
  String? filePath;
  int? classRoomId;
  String? className;

  Homework({
    required this.id,
    required this.isActive,
    required this.createUser,
    required this.createDate,
    required this.updateUser,
    required this.updateDate,
    this.schoolId,
    this.schoolName,
    this.teacherId,
    this.teacherName,
    this.lessonId,
    this.lessonName,
    this.filePath,
    this.classRoomId,
    this.className,
  });

  factory Homework.fromJson(Map<String, dynamic> json) => Homework(
        id: json["id"],
        isActive: json["isActive"],
        createUser: json["createUser"],
        createDate: DateTime.parse(json["createDate"]),
        updateUser: json["updateUser"],
        updateDate: DateTime.parse(json["updateDate"]),
        schoolId: json["schoolId"],
        schoolName: json["schoolName"],
        teacherId: json["teacherId"],
        teacherName: json["teacherName"],
        lessonId: json["lessonId"],
        lessonName: json["lessonName"],
        filePath: json["filePath"],
        classRoomId: json["classRoomId"],
        className: json["className"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "isActive": isActive,
        "createUser": createUser,
        "createDate": createDate.toIso8601String(),
        "updateUser": updateUser,
        "updateDate": updateDate.toIso8601String(),
        "schoolId": schoolId,
        "schoolName": schoolName,
        "teacherId": teacherId,
        "teacherName": teacherName,
        "lessonId": lessonId,
        "lessonName": lessonName,
        "filePath": filePath,
        "classRoomId": classRoomId,
        "className": className,
      };
}
