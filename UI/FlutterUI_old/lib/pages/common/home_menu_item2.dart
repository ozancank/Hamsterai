import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:mobile/styles/colors.dart';

class HomeMenuItem2 extends StatelessWidget {
  final String text;
  final Widget trailing;
  final String iconPath;
  final double? iconWidth;
  final void Function()? onTap;
  const HomeMenuItem2(
      {super.key,
      required this.text,
      required this.trailing,
      required this.iconPath,
      required this.onTap,
      this.iconWidth});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 4),
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color: Colors.transparent,
          border: Border.all(
            color: Colors.grey.withOpacity(0.3),
          ),
        ),
        child: ListTile(
          leading: SvgPicture.asset(
            iconPath,
            color: MyColors.primaryColor,
            // width: iconWidth ?? 30,
            // fit: BoxFit.contain,
          ),
          title: Text(
            text,
            style: Theme.of(context)
                .textTheme
                .bodyLarge!
                .copyWith(color: Colors.grey, fontSize: 20),
          ),
          trailing: trailing,
        ),
      ),
    );
  }
}
