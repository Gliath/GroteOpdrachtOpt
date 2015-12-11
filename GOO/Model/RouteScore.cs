using GOO.Utilities;
using System;

namespace GOO.Model
{
    /// <summary>
    /// Calculate the score of the given solution
    /// </summary>
    public static class RouteScore
    {
        public static double CalculateScore(string[] solution)
        {
            // Day;Truck;Sequence number;Order;
            double travelTime = 0.0;
            double penaltyTime = 0.0;
            // FilesInitializer._Orders for the list of orders
            Solution solutionToCheck = new Solution(solution);
            

            // Check for frequency
            foreach (Order order in FilesInitializer._Orders)
            {
                int numberOfOccurences = 0;
                for (int i = 0; i < solution.Length; i++)
                {
                    string[] info = solution[i].Split(';');

                    solutionToCheck.AddItem(info);
                }

                int orderFrequency;
                switch (order.Frequency)
	            {
		            case OrderFrequency.PWK1:
                        orderFrequency = 1;
                        break;
                    case OrderFrequency.PWK2:
                        orderFrequency = 2;
                     break;
                    case OrderFrequency.PWK3:
                        orderFrequency = 3;
                     break;
                    case OrderFrequency.PWK4:
                        orderFrequency = 4;
                     break;
                    case OrderFrequency.PWK5:
                        orderFrequency = 5;
                     break;
                    default:
                        orderFrequency = 0;
                     break;
	            }

                if(numberOfOccurences < orderFrequency) // Penalty time...
                    penaltyTime += orderFrequency * order.EmptyingTimeInMinutes * orderFrequency;
                else if (numberOfOccurences > orderFrequency) // Error time
                    Console.WriteLine("Something went wrong. The algorithm did the order to many times. {0} times instead of {1} times", numberOfOccurences, orderFrequency);
            }



            /*
            Collection<Order> orders = this.problem.getOrders();

            int accepted = orders.size() - 1;
            int totalOrders = accepted;
            for (Order o : orders)
            {
              if (o.id != 0)
              {
                if (!o.isValid(warnings))
                {
                  feasible = false;
                }
    
                if (o.declined)
                {
                  declinePenalty += o.freq * o.legingTijd * 3.0D;
                  accepted--;
                }
              }
            }


            warnings.addMessage("\n> Checking Routes:");
            for (int d = 0; d < 5; d++) {
              for (int t = 0; t < 2; t++)
              {
                double cumTime = 0.0D;
                int cumWaste = 0;
    
                Order[] route = this.sollution[t][d];
    
                int lastLocation = locationWasteDisposal;
    
                int trips = 0;
    
                warnings.addMessage("[" + (t + 1) + "][" + (d + 1) + "] Route of Truck " + (t + 1) + " on " + Order.getDay(d) + ":");
    

                for (int s = 0; s < route.length; s++)
                {
                  Order order = route[s];
      

                  cumTime += this.problem.getDistance(lastLocation, order.loc);
                  lastLocation = order.loc;
      
                  if (order.id == 0)
                  {
                    if (cumWaste == 0)
                    {
                      if (s == 0)
                      {
                        warnings.addWarning("!!! You don't have to dispose the garbage at the start of the schedule.");
                      }
                      else
                      {
                        warnings.addWarning("!!! Visiting the waste disposal location without carring garbage.");
                      }
          
                    }
                    else {
                      cumTime += 1800.0D;
                    }
        

                    if (cumWaste > 100000)
                    {
                      feasible = false;
                      warnings.addWarning("!!! The capacity of Truck " + (t + 1) + " on " + Order.getDay(d) + " is exceeded.");
                    }
        
                    double cumTimeRounded = (int)(cumTime * 10.0D) / 10.0D;
                    trips++;
                    warnings.addMessage("  Trip " + trips + " - load: " + cumWaste + "/" + 100000 + " l - time: " + cumTimeRounded + "/" + 43200 + " s (" + cumTime / 60.0D + " min)");
        
                    cumWaste = 0;

                  }
                  else
                  {
                    cumTime += order.legingTijd;
                    cumWaste += order.volume;
                  }
                }
    

                travelTime += cumTime;
                if (cumTime > 43200.0D)
                {
                  feasible = false;
                  warnings.addWarning("!!! The maximum service time of Truck " + (t + 1) + " on " + Order.getDay(d) + " is exceeded.");
                }
              }
            }
            */

            return travelTime + penaltyTime;
        }
    }
}