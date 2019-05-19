﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using WebsiteLaitBrasseur.BL;

namespace WebsiteLaitBrasseur.DAL
{
    public class ShippmentDAL
    {
        //Get connection string from web.config file and create sql connection
        SqlConnection connection = new SqlConnection(SqlDataAccess.ConnectionString);
        /// <summary>
        /// To Insert another Shipping company into the DB
        /// a status = 0 means the company is available
        /// a status = 1 means that the shipping service is deactivated.
        /// Customers don't have access to companies that are deactivated and as 
        /// such can't see this companies in the selection for a shipping service.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deliveryTime"></param>
        /// <param name="company"></param>
        /// <param name="arrivalDate"></param>
        /// <param name="postageDate"></param>
        /// <param name="cost"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int Insert(string type, int deliveryTime, string company, decimal cost, int status)
        {
            int result;
            //no need to explicitely set id as autoincrement is used
            string queryString = "INSERT INTO Shippment(dbo.Shippment.shipType, dbo.Shippment.estimatedTime, dbo.Shippment.shipCompany, " +
                "dbo.Shippment.shipCost, dbo.Shippment.status) " +
                "VALUES('@shipType', @deliveryTime, '@shipCompany', @shipCost, @status)";
            //query the last updated ID, which will be the id inserted by the above statement
            string queryAutoincID = "SELECT TOP(1) dbo.Shippment.shippingID FROM dbo.Shippment ORDER BY 1 DESC";
            try
            {
                //insert into database
                using (SqlCommand cmd = new SqlCommand(queryString, connection))
                {
                    cmd.Parameters.AddWithValue("@shipType", type);
                    cmd.Parameters.AddWithValue("@deliveryTime", deliveryTime);
                    cmd.Parameters.AddWithValue("@shipCompany", company);
                    cmd.Parameters.AddWithValue("@shipCost", cost);
                    cmd.Parameters.AddWithValue("@status", status);                    
                    connection.Open();
                    cmd.ExecuteNonQuery(); //returns the number of affected rows in the DB 
                }
                ///find the last manipulated id due to autoincrement and return it
                using (SqlCommand command = new SqlCommand(queryAutoincID, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    //won't need a while, since it will only retrieve one row
                    reader.Read();
                    //this is the id of the newly created data field
                    result = (Int32)reader["accountID"];
                    Debug.Print("ShippmentDAL: /Insert/ " + result.ToString());
                }
            }
            catch (Exception e)
            {
                result = 0;
                e.GetBaseException();
            }
            finally
            {
                connection.Close();
            }              
            return result;
        }

        /// <summary>
        /// Update the company status if status = 0 active
        ///                              status = 1 inactive
        /// Inactive services cant be selected by the customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int Update(int id, int status)
        {
            int result = 0;
            string queryString = "UPDATE dbo.Shippment SET status = @status WHERE shippingID = @id";
            try
            {
                //update into database where id = XY to status suspendet(false) or enabled(true) 
                //e.g. after three false log in attempts / upaied bills
                using (SqlCommand cmd = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@status", status);
                    result = cmd.ExecuteNonQuery(); //returns amount of affected rows if successfull
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }
            return result;
        }

        /// <summary>
        /// Update the shipping costs of a company
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shipCost"></param>
        /// <returns></returns>
        public int Update(int id, decimal shipCost)
        {
            int result = 0;
            string queryString = "UPDATE dbo.Shippment SET shipCost = @shipCost WHERE shippingID = @id";
            try
            {
                //update into database where id = XY to status suspendet(false) or enabled(true) 
                //e.g. after three false log in attempts / upaied bills
                using (SqlCommand cmd = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@shipCost", shipCost);
                    result = cmd.ExecuteNonQuery(); //returns amount of affected rows if successfull
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }
            return result;
        }

        /// <summary>
        /// Update all information of the shipping company
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deliveryTime"></param>
        /// <param name="company"></param>
        /// <param name="cost"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int Update(int id, string type, int deliveryTime, string company, decimal cost, int status)
        {
            int result = 0;
            string queryString = "UPDATE dbo.Shippment SET shipType = @type, estimatedTime = @deliveryTime, " +
                "shipCost = @shipCost, shipCompany = @shipCompany, status = @status WHERE shippingID = @id";
            try
            {
                //update into database where id = XY to status suspendet(false) or enabled(true) 
                //e.g. after three false log in attempts / upaied bills
                using (SqlCommand cmd = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@deliveryTime", deliveryTime);
                    cmd.Parameters.AddWithValue("@shipCost", cost);
                    cmd.Parameters.AddWithValue("@shipCompany", company);
                    cmd.Parameters.AddWithValue("@status", status);
                    result = cmd.ExecuteNonQuery(); //returns amount of affected rows if successfull
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }
            return result;
        }

        /// <summary>
        /// Find one specific shipper/delivery service by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShippmentDTO FindBy(int id)
        {
            ShippmentDTO deliverer;
            string queryString = "SELECT * FROM dbo.Shippment WHERE shippingID = @id";

            try
            {
                //find entry in database where id = XY
                using (SqlCommand cmd = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            deliverer = new ShippmentDTO();
                            deliverer.SetID((int)reader["shippingID"]);
                            deliverer.SetCompany(reader["shipCompany"].ToString());
                            deliverer.SetCost((decimal)reader["shipCost"]);
                            deliverer.SetDeliveryTime((int)reader["estimatedTime"]);
                            deliverer.SetStatus((int)reader["status"]);
                            deliverer.SetType(reader["shipType"].ToString());                       
                            //return product instance as data object 
                            Debug.Print("ShippmentDAL: /FindByID/ " + deliverer.ToString());
                            return deliverer;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
                Debug.Print(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// Find one specific shipping/delivery service
        /// by its company name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ShippmentDTO FindBy(string name)
        {
            ShippmentDTO deliverer;
            string queryString = "SELECT * FROM dbo.Shippment WHERE shipCompany = @name";

            try
            {
                //find entry in database where id = XY
                using (SqlCommand cmd = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@name", name);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            deliverer = new ShippmentDTO();
                            deliverer.SetID((int)reader["shippingID"]);
                            deliverer.SetCompany(reader["shipCompany"].ToString());
                            deliverer.SetCost((decimal)reader["shipCost"]);
                            deliverer.SetDeliveryTime((int)reader["estimatedTime"]);
                            deliverer.SetStatus((int)reader["status"]);
                            deliverer.SetType(reader["shipType"].ToString());
                            //return product instance as data object 
                            Debug.Print("ShippmentDAL: /FindByID/ " + deliverer.ToString());
                            return deliverer;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
                Debug.Print(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// Find all shipping services that are
        /// currently available status = 0 OR
        /// currently suspendet status = 1
        /// </summary>
        /// <param name="status"></param>
        /// <returns>List<ShippmentDTO</returns>
        public List<ShippmentDTO> FindAllBy(int status)
        {
            string queryString = "SELECT * FROM dbo.Shippment WHERE status = @status";
            List<ShippmentDTO> results = new List<ShippmentDTO>();
            ShippmentDTO deliverer;
            try
            {
                //find entry in database where id = XY
                using (SqlCommand cmd = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@status", status);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                deliverer = new ShippmentDTO();
                                deliverer.SetID((int)reader["shippingID"]);
                                deliverer.SetCompany(reader["shipCompany"].ToString());
                                deliverer.SetCost((decimal)reader["shipCost"]);
                                deliverer.SetDeliveryTime((int)reader["estimatedTime"]);
                                deliverer.SetStatus((int)reader["status"]);
                                deliverer.SetType(reader["shipType"].ToString());
                                //return product instance as data object 
                                Debug.Print("ShippmentDAL: /FindByID/ " + deliverer.ToString());
                                //add data objects to result-list 
                                results.Add(deliverer);
                            }
                            return results;
                        }
                        else
                        {
                            throw new EmptyRowException();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }
            return null;

        }

        /// <summary>
        /// Find all shipping companies that are in the DB
        /// not dependent if they are avaiable or not.
        /// </summary>
        /// <returns></returns>
        public List<ShippmentDTO> FindAll()
        {
            string queryString = "SELECT * FROM dbo.Shippment";
            List<ShippmentDTO> results = new List<ShippmentDTO>();
            ShippmentDTO deliverer;
            try
            {
                //find entry in database where id = XY
                using (SqlCommand cmd = new SqlCommand(queryString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                deliverer = new ShippmentDTO();
                                deliverer.SetID((int)reader["shippingID"]);
                                deliverer.SetCompany(reader["shipCompany"].ToString());
                                deliverer.SetCost((decimal)reader["shipCost"]);
                                deliverer.SetDeliveryTime((int)reader["estimatedTime"]);
                                deliverer.SetStatus((int)reader["status"]);
                                deliverer.SetType(reader["shipType"].ToString());
                                //return product instance as data object 
                                Debug.Print("ShippmentDAL: /FindByID/ " + deliverer.ToString());
                                //add data objects to result-list 
                                results.Add(deliverer);
                            }
                            return results;
                        }
                        else
                        {
                            throw new EmptyRowException();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.GetBaseException();
            }
            return null;
        }
    }
}