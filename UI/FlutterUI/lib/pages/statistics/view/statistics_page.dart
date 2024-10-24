import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:lottie/lottie.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/statistics/controller/statistics_controller.dart';
import 'package:mobile/pages/statistics/view/pie_chart.dart';
import 'package:mobile/styles/colors.dart';

class StatisticsPage extends StatefulWidget {
  const StatisticsPage({super.key});

  @override
  State<StatisticsPage> createState() => _StatisticsPageState();
}

class _StatisticsPageState extends State<StatisticsPage> {
  final statisticsController = Get.put(StatisticsController());
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: const CommonAppbar(title: 'Aylık Aktivite'),
      body: SingleChildScrollView(
        child: Column(
          children: [
            SizedBox(
              height: context.height * 0.02,
            ),
            Obx(() {
              if (statisticsController.loading.value) {
                return Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      Lottie.asset('assets/json/loading_line.json',
                          height: context.height * 0.2),
                      Text(
                        'İstatistikler yükleniyor...',
                        style: Theme.of(context)
                            .textTheme
                            .titleMedium!
                            .copyWith(color: MyColors.primaryColor),
                      ),
                    ],
                  ),
                );
              }

              var gain = statisticsController.studentGain;
              if (gain.value.forLessons.isEmpty) {
                return Center(
                  child: Text(
                    'Gösterilecek istatistik bulunamadı',
                    style: Theme.of(context).textTheme.bodyLarge,
                  ),
                );
              }
              return PieChartSample2(
                lessonDataModel: statisticsController.studentGain.value,
              );
            }),
          ],
        ),
      ),
    );
  }
}
