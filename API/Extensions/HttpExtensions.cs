﻿using API.Helpers;
using System.Text.Json;


namespace API.Extensions
{
	public static class HttpExtensions
	{
		public static void AddPaginationHeader(this HttpResponse response,PaginationHeader header)
		{
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};

			response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
			response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
		}

		public static void AddPaginationHeader(this HttpResponse response,int currentPage, int itemsPerPage,
			int totalItems,int totalPages)
		{
			var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};

			response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
			response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
		}
	}
}
