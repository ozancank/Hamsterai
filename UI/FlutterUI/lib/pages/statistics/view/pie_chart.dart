import 'dart:math';

import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/pages/statistics/model/gain_model.dart';
import 'package:mobile/pages/statistics/view/lesson_gain_view.dart';
import 'package:mobile/styles/colors.dart';

class PieChartSample2 extends StatefulWidget {
  final LessonDataModel lessonDataModel;
  const PieChartSample2({super.key, required this.lessonDataModel});

  @override
  State<PieChartSample2> createState() => _PieChartSample2State();
}

class _PieChartSample2State extends State<PieChartSample2> {
  double totalValue = 0;
  int lessonValue = 0;
  final List<Color> predefinedColors = [
    const Color(0xFFF2762E),
    const Color(0xFFF29422),
    const Color.fromARGB(255, 199, 177, 137),
    const Color(0xFFF2D06B),
    const Color(0xFFF0231A),
    const Color(0xFFF29B6A),
    Colors.green,
    Colors.blue,
    Colors.orange,
    Colors.purple,
    Colors.cyan,
    Colors.teal,
    Colors.indigo,
    Colors.pink,
    Colors.amber,
  ];

  int colorIndex = 0;
  int allGainNumber = 0;
  int touchedIndex = -1;
  final Map<String, Color> lessonColors = {};
  void assignLessonColor(String lesson) {
    if (!lessonColors.containsKey(lesson)) {
      final color = predefinedColors[colorIndex];
      lessonColors[lesson] = color;
      colorIndex = (colorIndex + 1) % predefinedColors.length;
    }
  }

  @override
  Widget build(BuildContext context) {
    final indicators = widget.lessonDataModel.forLessons.keys.map((lesson) {
      if (!lessonColors.containsKey(lesson)) {
        assignLessonColor(lesson);
      }

      final gainsForLesson =
          widget.lessonDataModel.forLessonGains[lesson] ?? {};
      final totalGain =
          gainsForLesson.values.fold(0, (sum, item) => sum + item);
      allGainNumber += totalGain;
      return Container(
        padding: const EdgeInsets.symmetric(vertical: 4),
        child: Indicator(
          color: lessonColors[lesson]!,
          text: lesson,
          isSquare: true,
          lessonDataModel: widget.lessonDataModel,
          totalValue: totalValue,
          lessonValue: lessonValue,
          totalGain: totalGain,
        ),
      );
    }).toList();

    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Align(
          alignment: Alignment.center,
          child: Container(
            padding: EdgeInsets.only(
              top: context.height * 0.002,
              bottom: context.height * 0.002,
              left: context.width * 0.005,
              right: context.width * 0.05,
            ),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(30),
              border: Border.all(color: MyColors.primaryColor),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  padding: const EdgeInsets.all(10),
                  alignment: Alignment.center,
                  decoration: const BoxDecoration(
                      shape: BoxShape.circle, color: MyColors.primaryColor),
                  child: SvgPicture.asset(
                    AssetsConstant.chart,
                    color: Colors.white,
                  ),
                ),
                const SizedBox(width: 10),
                Text(
                  'Toplam Soru: $allGainNumber',
                  style: Theme.of(context).textTheme.titleMedium!.copyWith(
                        color: MyColors.primaryColor,
                        fontWeight: FontWeight.w500,
                      ),
                ),
              ],
            ),
          ),
        ),
        SizedBox(height: context.height * 0.06),
        Stack(
          children: [
            SizedBox(
              height: context.height * 0.25,
              child: PieChart(
                PieChartData(
                  borderData: FlBorderData(show: false),
                  sectionsSpace: 1,
                  centerSpaceRadius: 70,
                  sections: showingSections(widget.lessonDataModel),
                ),
              ),
            ),
          ],
        ),
        SizedBox(height: context.height * 0.06),
        Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: indicators,
        ),
      ],
    );
  }

  List<PieChartSectionData> showingSections(LessonDataModel lessonDataModel) {
    totalValue = lessonDataModel.forLessons.values.fold(0, (a, b) => a + b);

    return lessonDataModel.forLessons.entries.map((entry) {
      final lessonName = entry.key;
      lessonValue = entry.value;

      final gainsForLesson =
          widget.lessonDataModel.forLessonGains[lessonName] ?? {};
      final totalGain =
          gainsForLesson.values.fold(0, (sum, item) => sum + item);

      final isTouched =
          lessonDataModel.forLessons.keys.toList().indexOf(lessonName) ==
              touchedIndex;
      final fontSize = isTouched ? 25.0 : 13.0;
      final radius = isTouched ? 60.0 : 50.0;
      const shadows = [Shadow(color: Colors.black, blurRadius: 2)];
      final lessonColor = lessonColors[lessonName]!;
      return PieChartSectionData(
        color: lessonColor,
        value: (lessonValue / totalValue) * 100,
        // title: '${((totalGain))} Soru\n $lessonName',
        title: lessonName,
        radius: radius,
        titleStyle: TextStyle(
          fontSize: fontSize,
          fontWeight: FontWeight.bold,
          color: Colors.white,
          shadows: shadows,
        ),
      );
    }).toList();
  }
}

class Indicator extends StatelessWidget {
  const Indicator({
    super.key,
    required this.color,
    required this.text,
    required this.isSquare,
    this.size = 16,
    this.textColor,
    required this.lessonDataModel,
    required this.totalValue,
    required this.lessonValue,
    required this.totalGain,
  });
  final Color color;
  final String text;
  final bool isSquare;
  final double size;
  final Color? textColor;
  final LessonDataModel lessonDataModel;
  final double totalValue;
  final int lessonValue;
  final int totalGain;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () {
        Get.to(() =>
            LessonGainView(lessonDataModel: lessonDataModel, lesson: text));
      },
      child: Container(
        width: double.infinity,
        padding: EdgeInsets.symmetric(
            vertical: 10, horizontal: context.width * 0.04),
        margin: EdgeInsets.symmetric(
          horizontal: context.width * 0.15,
        ),
        decoration: BoxDecoration(
          color: color,
          borderRadius: BorderRadius.circular(30),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceAround,
          children: [
            SvgPicture.asset(AssetsConstant.chart, color: Colors.white),
            // Text(
            //   '%${((lessonValue / totalValue) * 100).toStringAsFixed(1)}',
            //   style: TextStyle(
            //     fontSize: 16,
            //     fontWeight: FontWeight.bold,
            //     color: textColor ?? Colors.white,
            //   ),
            // ),
            Text(
              text,
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.bold,
                color: textColor ?? Colors.white,
              ),
            ),
            Text(
              '$totalGain Soru',
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.bold,
                color: textColor ?? Colors.white,
              ),
            ),
            SvgPicture.asset(
              AssetsConstant.rightBack,
              color: Colors.white,
              height: context.height * 0.03,
            ),
          ],
        ),
      ),
    );
  }
}
