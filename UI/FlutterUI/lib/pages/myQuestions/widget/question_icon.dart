import 'package:flutter/material.dart';
import 'package:get/get_rx/src/rx_types/rx_types.dart';
import 'package:mobile/pages/scan/model/get_question_model.dart';

class QuestionIcon extends StatelessWidget {
  final Question question;
  const QuestionIcon({
    super.key,
    required this.question,
    required this.questions,
  });

  final RxList questions;

  @override
  Widget build(BuildContext context) {
    IconData iconData;

    switch (question.status) {
      case 0:
        iconData = Icons.hourglass_bottom_sharp;
        break;
      case 1:
        iconData = Icons.hourglass_bottom_sharp;
        break;
      case 2:
        iconData = Icons.done;
        break;
      case 3:
        iconData = Icons.close;
        break;
      case 4:
        iconData = Icons.hourglass_bottom_sharp;
        break;
      case 5:
        iconData = Icons.hourglass_bottom_sharp;
        break;
      case 6:
        iconData = Icons.hourglass_bottom_sharp;
        break;
      case 7:
        iconData = Icons.close;
        break;
      case 8:
        iconData = Icons.hourglass_bottom_sharp;
        break;
      default:
        iconData = Icons.hourglass_bottom_sharp;
        break;
    }
    return Icon(
      iconData,
      color: Colors.white,
    );
  }
}
