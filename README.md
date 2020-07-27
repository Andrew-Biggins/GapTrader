# GapTrader

Gap Trader is a tool for finding and testing day-trading strategies on the financial markets. 
Since the strategies are based around the opening and closing cash price gaps it is best used with the major stock market indices such as the FTSE, Dax and Dow Jones. 
Furthermore, keeping costs low in the form of tight buy/sell spreads is essential when it comes to finding profitable short-term trading strategies.

The Gap Fill Strategy

The strategy uses Fibonacci levels derived from the daily open/close gap to generate entry and target levels. 
There are two variations of the strategy; one trades into the gap and the other trades out of it.

Trading into the gap involves aiming to enter the market at a selected Fibonacci extension level with a target at a selected Fibonacci retracement level.

Trading out of the gap is the opposite. It aims to enter at a selected Fibonacci retracement level with a target at a selected Fibonacci extension level.

In both strategies a stop-loss of either a fixed points size or percentage of the gap size can be selected. Whether the stop-loss trails the market price and by how much can also be chosen.

Other important details to note are:

Each strategy will only make a maximum of one trade per day
If the entry level is not reached on any given day there is no trade
Trades are not held from one day to the next, if the target or stop-loss is not hit they are closed at the closing price of the final minute candle of the day regardless of any profit or loss
