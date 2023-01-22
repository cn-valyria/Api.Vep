using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AutoMapper;
using System.Web.Http;
using System.Linq;
using Repository;

namespace api;

public class ListsEntryPoint
{
    private readonly IListsRepository _listsRepository;
    private readonly IMapper _mapper;

    public ListsEntryPoint(IListsRepository listsRepository, IMapper mapper)
        => (_listsRepository, _mapper) = (listsRepository, mapper);

    [FunctionName(nameof(GetAllLists))]
    public async Task<IActionResult> GetAllLists(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lists")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation($"Beginning execution for {nameof(GetAllLists)} method...");

        try
        {
            var results = await _listsRepository.GetAidListsAsync();

            log.LogInformation($"{nameof(GetAllLists)} method completed successfully. Found {results.Count} results.");

            return new OkObjectResult(results.Select(_mapper.Map<AidList>));
        }
        catch (Exception e)
        {
            var wrapperException = new Exception($"Unexpected error occurred while executing {nameof(GetAllLists)}", e);
            log.LogError(e, wrapperException.Message);
            return new ExceptionResult(wrapperException, true);
        }
    }
}
