$votingweb = Get-ServiceFabricService -ApplicationName fabric:/Voting -ServiceName fabric:/Voting/VotingWeb
$votingdata = Get-ServiceFabricService -ApplicationName fabric:/Voting -ServiceName fabric:/Voting/VotingData


if (!$votingweb)
{
    New-ServiceFabricService -ApplicationName fabric:/Voting -ServiceName fabric:/Voting/VotingWeb -ServiceTypeName VotingWebType -Stateless -PartitionSchemeSingleton -InstanceCount 1 -ServicePackageActivationMode ExclusiveProcess
}
