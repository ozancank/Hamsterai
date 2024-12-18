import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';

// ignore: must_be_immutable
class CommonListTile extends StatelessWidget {
  final String iconPath;
  final String text;
  void Function()? onTap;
  final Widget? icon;
  CommonListTile({
    super.key,
    required this.iconPath,
    required this.text,
    required this.onTap,
    this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(bottom: context.dynamicHeight * 0.02),
      child: ListTile(
          onTap: onTap,
          leading: icon ?? SvgPicture.asset(iconPath),
          title: Text(
            text,
            style: Theme.of(context).textTheme.titleMedium!.copyWith(
                fontWeight: FontWeight.w300, color: const Color(0xFF27364E)),
          ),
          trailing: SvgPicture.asset(AssetsConstant.arrow)),
    );
  }
}
