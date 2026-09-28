namespace Wishlist.Core.Enums;

/// <summary>
/// These class explained what types are we supporting to creating the list
/// TYPE_WISH_LIST = Wishlist
/// TYPE_SHOPPING_LIST = Shopping Cart List
/// TYPE_GIFT_REGISTRY = Gift Registry
/// TYPE_CUSTOM_1, TYPE_CUSTOM_2, TYPE_CUSTOM_3 = These custom created by customer
/// </summary>
public enum ProductListType
{
    TYPE_CUSTOM_1 = 100,
    TYPE_CUSTOM_2 = 101,
    TYPE_CUSTOM_3 = 102,
    TYPE_GIFT_REGISTRY = 11,
    TYPE_SHOPPING_LIST = 12,
    TYPE_WISH_LIST = 10
}