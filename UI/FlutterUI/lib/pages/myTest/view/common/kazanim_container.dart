import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/styles/colors.dart';

class KazanimContainer extends StatelessWidget {
  final Color bgColor;
  final BoxBorder? border;
  final String assetName;
  final String text;
  final Color textColor;
  final Color? iconBgColor;
  final bool isStartButton;

  const KazanimContainer({
    super.key,
    this.bgColor = Colors.transparent,
    required this.border,
    this.assetName = AssetsConstant.kazanimlar,
    required this.text,
    required this.textColor,
    this.iconBgColor,
    this.isStartButton = false,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: isStartButton
          ? EdgeInsets.symmetric(horizontal: context.width * 0.02, vertical: 5)
          : const EdgeInsets.all(5),
      margin: EdgeInsets.zero,
      decoration: BoxDecoration(
        color: bgColor,
        borderRadius: BorderRadius.circular(40),
        border: border ?? Border.all(color: const Color(0xFFADB4C0)),
      ),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              margin: const EdgeInsets.symmetric(vertical: 1),
              alignment: Alignment.center,
              padding: const EdgeInsets.all(5),
              decoration: BoxDecoration(
                color: iconBgColor ?? MyColors.primaryColor,
                shape: BoxShape.circle,
              ),
              child: SvgPicture.asset(assetName),
            ),
            SizedBox(width: context.width * 0.02),
            Text(
              text,
              style: Theme.of(context).textTheme.titleSmall!.copyWith(
                  color: textColor,
                  fontWeight:
                      isStartButton ? FontWeight.bold : FontWeight.w400),
            ),
            SizedBox(width: context.width * 0.02),
          ],
        ),
      ),
    );
  }
}
