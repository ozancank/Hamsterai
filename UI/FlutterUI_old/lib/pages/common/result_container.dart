import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/styles/colors.dart';

class ResultContainer extends StatelessWidget {
  final String text;
  final IconData iconData;
  final void Function()? onTap;
  final Color? bgColor;
  final Color? iconColor;
  final Color? borderColor;
  final String containerName;
  final Map<String, Map<String, int>> gainNames;

  const ResultContainer({
    super.key,
    required this.text,
    required this.iconData,
    required this.onTap,
    required this.gainNames,
    this.bgColor,
    this.iconColor,
    this.borderColor,
    required this.containerName,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        GestureDetector(
          onTap: onTap,
          child: Container(
            margin: const EdgeInsets.symmetric(vertical: 2),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(10),
              color: containerName == 'dogru'
                  ? const Color(0xFF79A637)
                  : containerName == 'yanlis'
                      ? const Color(0xFFDD5050)
                      : Colors.transparent,
              border: Border.all(
                color: containerName == 'toplam'
                    ? MyColors.primaryColor
                    : containerName == 'bos'
                        ? const Color(0xFFD9D9D9)
                        : Colors.transparent,
              ),
            ),
            child: ListTile(
              leading: containerName == 'dogru'
                  ? SvgPicture.asset(
                      AssetsConstant.correctCount,
                      color: Colors.white,
                    )
                  : containerName == 'yanlis'
                      ? SvgPicture.asset(
                          AssetsConstant.incorrectCount,
                          color: Colors.white,
                        )
                      : Icon(
                          iconData,
                          color: containerName == 'dogru' ||
                                  containerName == 'yanlis'
                              ? Colors.white
                              : containerName == 'bos'
                                  ? const Color(0xFF808080)
                                  : MyColors.primaryColor,
                        ),
              title: Text(
                text,
                style: Theme.of(context).textTheme.bodyLarge!.copyWith(
                    color: containerName == 'dogru' || containerName == 'yanlis'
                        ? Colors.white
                        : Colors.grey),
              ),
            ),
          ),
        ),
        const SizedBox(height: 10),
        if (containerName == 'toplam')
          Column(
            children: gainNames.entries.map((entry) {
              String kazanimAdi = entry.key;
              Map<String, int> correctIncorrect = entry.value;
              int correctCount = correctIncorrect['correct'] ?? 0;
              int incorrectCount = correctIncorrect['incorrect'] ?? 0;
              int totalGain = correctCount + incorrectCount;

              return Column(
                children: [
                  Row(
                    children: [
                      SizedBox(width: context.width * 0.05),
                      SvgPicture.asset(AssetsConstant.gainArrow),
                      const SizedBox(width: 5),
                      Expanded(
                        child: Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 10, vertical: 5),
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(5),
                            border: Border.all(
                              color: borderColor ?? MyColors.primaryColor,
                              width: 0.5,
                            ),
                          ),
                          child: Text(
                            '$totalGain - Adet - $kazanimAdi',
                            style: Theme.of(context)
                                .textTheme
                                .bodyMedium!
                                .copyWith(color: const Color(0xFF808080)),
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                ],
              );
            }).toList(),
          ),
        if (containerName == 'dogru')
          Column(
            children: gainNames.entries.where((entry) {
              int correctCount = entry.value['correct'] ?? 0;
              return correctCount > 0;
            }).map((entry) {
              String kazanimAdi = entry.key;
              Map<String, int> correctIncorrect = entry.value;
              int correctCount = correctIncorrect['correct'] ?? 0;
              int totalGain = correctCount;

              return Column(
                children: [
                  Row(
                    children: [
                      SizedBox(width: context.width * 0.05),
                      SvgPicture.asset(
                        AssetsConstant.gainArrow,
                        color: const Color(0xFF79A637),
                      ),
                      const SizedBox(width: 5),
                      Expanded(
                        child: Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 10, vertical: 5),
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(5),
                            border: Border.all(
                              color: const Color(0xFF79A637),
                              width: 0.5,
                            ),
                          ),
                          child: Text(
                            '$totalGain - Adet - $kazanimAdi',
                            style: Theme.of(context)
                                .textTheme
                                .bodyMedium!
                                .copyWith(color: const Color(0xFF808080)),
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                ],
              );
            }).toList(),
          ),
        if (containerName == 'yanlis')
          Column(
            children: gainNames.entries.where((entry) {
              int incorrectCount = entry.value['incorrect'] ?? 0;
              return incorrectCount > 0;
            }).map((entry) {
              String kazanimAdi = entry.key;
              Map<String, int> correctIncorrect = entry.value;
              int incorrectCount = correctIncorrect['incorrect'] ?? 0;
              int totalGain = incorrectCount;

              return Column(
                children: [
                  Row(
                    children: [
                      SizedBox(width: context.width * 0.05),
                      SvgPicture.asset(
                        AssetsConstant.gainArrow,
                        color: const Color(0xFFDD5050),
                      ),
                      const SizedBox(width: 5),
                      Expanded(
                        child: Container(
                          width: context.width * 0.77,
                          padding: const EdgeInsets.symmetric(
                              horizontal: 10, vertical: 5),
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(5),
                            border: Border.all(
                              color: const Color(0xFFDD5050),
                              width: 0.5,
                            ),
                          ),
                          child: Text(
                            '$totalGain - Adet - $kazanimAdi',
                            style: Theme.of(context)
                                .textTheme
                                .bodyMedium!
                                .copyWith(color: const Color(0xFF808080)),
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                ],
              );
            }).toList(),
          ),
        if (containerName == 'bos')
          Column(
            children: gainNames.entries.where((entry) {
              int emptyCount = entry.value['count'] ?? 0;
              return emptyCount > 0;
            }).map((entry) {
              String gainName = entry.key;
              int emptyCount = entry.value['count'] ?? 0;

              return Column(
                children: [
                  Row(
                    children: [
                      SizedBox(width: context.width * 0.05),
                      SvgPicture.asset(
                        AssetsConstant.gainArrow,
                        color: const Color(0xFFD9D9D9),
                      ),
                      const SizedBox(width: 5),
                      Expanded(
                        child: Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 10, vertical: 5),
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(5),
                            border: Border.all(
                              color: const Color(0xFFD9D9D9),
                              width: 0.5,
                            ),
                          ),
                          child: Text(
                            '$emptyCount - Adet - $gainName',
                            style: Theme.of(context)
                                .textTheme
                                .bodyMedium!
                                .copyWith(color: const Color(0xFF808080)),
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                ],
              );
            }).toList(),
          ),
      ],
    );
  }
}
