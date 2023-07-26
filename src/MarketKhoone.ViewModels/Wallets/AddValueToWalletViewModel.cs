using MarketKhoone.Common.Attributes;
using MarketKhoone.Common.Constants;
using MarketKhoone.Entities.Enums.Order;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MarketKhoone.ViewModels.Wallets;

/// <summary>
/// افزودن مقدار به کیف پول
/// </summary>
public class AddValueToWalletViewModel
{
    public PaymentGateway PaymentGateway { get; set; }

    [HiddenInput]
    [Display(Name = "مبلغ")]
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [Range(10000, 20000000, ErrorMessage = "مبلغ وارد شده باید بین ۱۰/۰۰۰ تا ۲۰/۰۰۰/۰۰۰ تومان باشد")]
    [DivisibleBy10]
    public int Value { get; set; }
}