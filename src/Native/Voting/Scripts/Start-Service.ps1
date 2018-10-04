$votingweb = Get-ServiceFabricService -ApplicationName fabric:/Voting -ServiceName fabric:/Voting/VotingWeb
$votingdata = Get-ServiceFabricService -ApplicationName fabric:/Voting -ServiceName fabric:/Voting/VotingData


if (!$votingweb)
{
    New-ServiceFabricService -ApplicationName fabric:/Voting -ServiceName fabric:/Voting/VotingWeb -ServiceTypeName VotingWebType -Stateless -PartitionSchemeSingleton -InstanceCount 1 -ServicePackageActivationMode ExclusiveProcess
}

if (!$votingdata)
{
    New-ServiceFabricService -ApplicationName fabric:/Voting -ServiceName fabric:/Voting/VotingData -ServiceTypeName VotingDataType -Stateful -PartitionSchemeUniformInt64 -LowKey 0 -HighKey 25 -PartitionCount 1 -HasPersistedState -MinReplicaSetSize 3 -TargetReplicaSetSize 3 -ServicePackageActivationMode ExclusiveProcess
}