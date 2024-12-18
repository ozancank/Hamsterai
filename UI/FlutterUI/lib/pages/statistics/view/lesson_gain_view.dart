import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/statistics/model/gain_model.dart';
import 'package:mobile/styles/colors.dart';

class LessonGainView extends StatefulWidget {
  final LessonDataModel lessonDataModel;
  final String lesson;
  const LessonGainView(
      {super.key, required this.lessonDataModel, required this.lesson});

  @override
  State<LessonGainView> createState() => _LessonGainViewState();
}

class _LessonGainViewState extends State<LessonGainView> {
  @override
  Widget build(BuildContext context) {
    final gainsForLesson =
        widget.lessonDataModel.forLessonGains[widget.lesson] ?? {};
    final totalGain = gainsForLesson.values.fold(0, (sum, item) => sum + item);

    return Scaffold(
      backgroundColor: Colors.white,
      appBar:  CommonAppbar2(title: 'Kazanımlar'),
      body: SingleChildScrollView(
        child: Column(
          children: [
            SizedBox(
              height: context.height * 0.01,
            ),
            Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('${widget.lesson} dersi için kazanımlar',
                      style: Theme.of(context).textTheme.titleLarge!.copyWith(
                            color: MyColors.primaryColor,
                            fontWeight: FontWeight.bold,
                          )),
                  const SizedBox(height: 16),
                  ...gainsForLesson.entries.map((entry) {
                    final gainName = entry.key;
                    final gainValue = entry.value;
                    final progressValue =
                        totalGain == 0 ? 0 : gainValue / totalGain;

                    return Padding(
                      padding: const EdgeInsets.symmetric(vertical: 8.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            gainName.length > 50
                                ? '${gainName.substring(0, 50)}...'
                                : gainName,
                            style: Theme.of(context)
                                .textTheme
                                .bodyLarge!
                                .copyWith(fontWeight: FontWeight.bold),
                          ),
                          const SizedBox(height: 4),
                          LinearProgressIndicator(
                            minHeight: context.height * 0.01,
                            borderRadius: BorderRadius.circular(10),
                            value:
                                progressValue.toDouble(), // Oranlanmış değer.
                            backgroundColor: Colors.grey[300],
                            valueColor: const AlwaysStoppedAnimation<Color>(
                                MyColors.primaryColor),
                          ),
                          const SizedBox(height: 4),
                          Text('Kazanım: $gainValue / $totalGain'),
                        ],
                      ),
                    );
                  }),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
