﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// represent ability to take loans/deposits
/// </summary>
public class Agent
{
    /// <summary>
    /// Must be filled together with wallet
    /// </summary>
    public Value moneyIncomethisTurn = new Value(0);
    public Value cash = new Value(0);
    /// <summary>
    /// could be null
    /// </summary>
    public Bank bank;

    public Value loans = new Value(0);
    public Value deposits = new Value(0);

    //public Agent(Bank bank) : base(0f, bank)
    //{

    //}
    public Agent(float inAmount, Bank bank)
    {
        cash.set(inAmount);
        this.bank = bank;
    }
    public Value getMoneyAvailable()
    {
        //return cash.addOutside(deposits);
        if (bank == null)
            return new Value(cash);
        else
            return cash.addOutside(bank.howMuchDepositCanReturn(this));
    }
    //new internal bool canPay(Value howMuchPay)
    //{
    //    return getMoneyTotal().isBiggerOrEqual(howMuchPay);
    //}
    ///// <summary>
    ///// depreciated
    ///// </summary>    
    //private bool canPay(float howMuchPay)
    //{
    //    throw new DontUseThatMethod();
    //}
    //internal CountryWallet getCountryWallet()
    //{
    //    if (this is Country)
    //        return wallet as CountryWallet;
    //    else
    //        return null;
    //}
    //todo should be Value
    //public float getLoans()
    //{
    //    if (loans.get() > 0f)
    //        return loans.get();
    //    else
    //        return 0f;
    //}
    //public float getDeposits()
    //{
    //    if (loans.get() > 0f)
    //        return 0f;
    //    else
    //        return loans.get() * -1f;
    //}
    //***************
    internal bool CanAfford(Storage need)
    {
        if (need.get() == HowMuchCanAfford(need).get())
            return true;
        else
            return false;
    }

    internal bool CanAfford(PrimitiveStorageSet need)
    {
        foreach (Storage stor in need)
        {
            if (HowMuchCanAfford(stor).get() < stor.get())
                return false;
        }
        return true;
    }
    /// <summary>WARNING! Can overflow if money > cost of need. use CanAfford before </summary>
    //internal Value HowMuchCanNotAfford(PrimitiveStorageSet need)
    //{
    //    return new Value(Game.market.getCost(need).get() - this.cash.get());
    //}
    /// <summary>WARNING! Can overflow if money > cost of need. use CanAfford before </summary>
    //internal Value HowMuchCanNotAfford(float need)
    //{
    //    return new Value(need - this.cash.get());
    //}
    /// <summary>WARNING! Can overflow if money > cost of need. use CanAfford before </summary>
    internal Value HowMuchMoneyCanNotPay(Value need)
    {
        //return new Value(need - this.cash.get());
        //return need.subtractOutside(cash);
        return need.subtractOutside(getMoneyAvailable());
    }
    //internal Value HowMuchMoneyCanNotPay(Value value)
    //{
    //    return new Value(value.get() - this.cash.get());
    //}
    /// <summary>WARNING! Can overflow if money > cost of need. use CanAfford before </summary>
    //internal Value HowMuchCanNotAfford(Storage need)
    //{
    //    return new Value(Game.market.getCost(need) - this.cash.get());
    //}
    internal Storage HowMuchCanAfford(Storage need)
    {    

        Value cost = Game.market.getCost(need);
        if (canPay(cost))
            return new Storage(need);
        else
            return new Storage(need.getProduct(), getMoneyAvailable().divideOutside(
                Game.market.findPrice(need.getProduct())
                ));
    }

    //private float get()
    //{
    //    throw new NotImplementedException();
    //}

    //internal bool canPay(Value howMuchPay)
    //{
    //    if (this.cash.get() >= howMuchPay.get())
    //        return true;
    //    else return false;
    //}
    //internal bool canPay(float howMuchPay)
    //{
    //    if (this.cash.get() >= howMuchPay)
    //        return true;
    //    else
    //        return false;
    //}
    internal bool canPay(Value howMuchPay)
    {
        return getMoneyAvailable().isBiggerOrEqual(howMuchPay);
    }
    internal bool canPayInCash(Value howMuchPay)
    {
        return cash.isBiggerOrEqual(howMuchPay);
    }

    public float getCash()
    {
        return cash.get();
    }

    //internal void pay(Wallet whom, float howMuch)
    //{
    //    if (canPay(howMuch))
    //    {
    //        whom.haveMoney.add(howMuch);
    //        whom.moneyIncomethisTurn.add(howMuch);
    //        this.haveMoney.subtract(howMuch);

    //    }
    //    else
    //        Debug.Log("Failed payment in wallet");
    //}
    /// <summary>
    /// checks inside. Wouldn't pay if can't
    /// </summary>    
    public bool payWithoutRecord(Agent whom, Value howMuch)
    {
        if (canPay(howMuch))
        {
            if (!canPayInCash(howMuch) && bank != null)
                //bank.giveMoney(this, new Value(howMuch.get() - cash.get()));
                bank.giveLackingMoney(this, howMuch.multipleOutside(5));

            whom.cash.add(howMuch);
            this.cash.subtract(howMuch);
            return true;
        }
        else
        {
            Debug.Log("Not enough money to pay in Agent.payWithoutRecord");
            return false;
        }

    }
    /// <summary>
    /// checks inside. Wouldn't pay if can't
    /// </summary>    
    public bool pay(Agent whom, Value howMuch)
    {
        if (payWithoutRecord(whom, howMuch))
        {
            whom.moneyIncomethisTurn.add(howMuch);
            return true;
        }
        else
            return false;
    }
    internal void sendAllAvailableMoney(Agent whom)
    {
        if (bank != null)
            bank.returnAllMoney(this);
        whom.cash.add(this.cash);
        whom.moneyIncomethisTurn.add(this.cash);
        this.cash.set(0);
    }
    internal void sendAllAvailableMoneyWithoutRecord(Agent whom)
    {
        if (bank != null)
            bank.returnAllMoney(this);
        whom.cash.add(this.cash);
        //whom.moneyIncomethisTurn.add(this.cash);
        this.cash.set(0);
    }
    public void ConvertFromGoldAndAdd(Value gold)
    {
        float coins = gold.get() * Options.goldToCoinsConvert;
        this.cash.add(coins);
        this.moneyIncomethisTurn.add(coins);
        gold.set(0);

    }

    override public string ToString()
    {
        return cash.get() + " coins";
    }
}