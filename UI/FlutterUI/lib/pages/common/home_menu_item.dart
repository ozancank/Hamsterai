import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:mobile/core/extensions/size_extension.dart';

class HomeMenuItem extends StatelessWidget {
  final Color bgColor;
  final String text;
  final String iconPath;
  final void Function()? onTap;
  const HomeMenuItem({
    super.key,
    required this.bgColor,
    required this.text,
    required this.iconPath,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: EdgeInsets.symmetric(vertical: context.dynamicHeight * 0.005),
        padding: EdgeInsets.only(
            left: context.dynamicWidth * 0.01,
            top: 4,
            bottom: 4,
            right: context.dynamicWidth * 0.05),
        decoration: BoxDecoration(
          color: bgColor,
          borderRadius: BorderRadius.circular(40),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              alignment: Alignment.center,
              padding: const EdgeInsets.all(10),
              decoration: const BoxDecoration(
                shape: BoxShape.circle,
                color: Colors.white,
              ),
              child: SvgPicture.asset(
                iconPath,
                height: context.dynamicHeight * 0.04,
                color: text == 'Testlerim' ? Colors.grey : null,
              ),
            ),
            SizedBox(
              width: context.dynamicWidth * 0.02,
            ),
            Text(
              text,
              style: Theme.of(context).textTheme.headlineLarge!.copyWith(
                    fontWeight: FontWeight.w800,
                    color: Colors.white,
                    fontSize: 20,
                  ),
            ),
            SizedBox(
              width: context.dynamicWidth * 0.02,
            ),
          ],
        ),
      ),
    );
  }
}
