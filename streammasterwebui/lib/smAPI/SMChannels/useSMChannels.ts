import { FieldData, GetApiArgument, PagedResponse, QueryHookResult,SMChannelDto,DefaultAPIResponse,QueryStringParameters,SMStreamSMChannelRequest,SMChannelRankRequest } from '@lib/apiDefs';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { CreateSMChannelFromStream } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMChannels } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteAllSMChannelsFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { AddSMStreamToSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { RemoveSMStreamFromSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SetSMStreamRanks } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { fetchGetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsFetch';
import { clearSMChannels, updateSMChannels } from '@lib/smAPI/SMChannels/SMChannelsSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<SMChannelDto> | undefined> {}

interface SMChannelDtoResult extends ExtendedQueryHookResult {
  createSMChannelFromStream: (streamId: string) => Promise<DefaultAPIResponse | null>;
  deleteSMChannels: (smchannelIds: number[]) => Promise<DefaultAPIResponse | null>;
  deleteSMChannel: (smchannelId: number) => Promise<DefaultAPIResponse | null>;
  deleteAllSMChannelsFromParameters: (Parameters: QueryStringParameters) => Promise<DefaultAPIResponse | null>;
  addSMStreamToSMChannel: (request: SMStreamSMChannelRequest) => Promise<DefaultAPIResponse | null>;
  removeSMStreamFromSMChannel: (request: SMStreamSMChannelRequest) => Promise<DefaultAPIResponse | null>;
  setSMStreamRanks: (requests: SMChannelRankRequest[]) => Promise<DefaultAPIResponse | null>;
  setSMChannelsField: (fieldData: FieldData) => void;
  refreshSMChannels: () => void;
}

const useSMChannels = (params?: GetApiArgument | undefined): SMChannelDtoResult => {
  const query = JSON.stringify(params);
  const dispatch = useAppDispatch();

  const data = useAppSelector((state) => state.SMChannels.data[query]);
  const isLoading = useAppSelector((state) => state.SMChannels.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.SMChannels.isError[query] ?? false);
  const error = useAppSelector((state) => state.SMChannels.error[query] ?? '');

  useEffect(() => {
    if (params === undefined || data !== undefined) return;
    dispatch(fetchGetPagedSMChannels(query));
  }, [data, dispatch, params, query]);

  const setSMChannelsField = (fieldData: FieldData): void => {
    dispatch(updateSMChannels({ fieldData: fieldData }));
  };

  const refreshSMChannels = (): void => {
    dispatch(clearSMChannels());
  };

  const createSMChannelFromStream = (streamId: string): Promise<DefaultAPIResponse | null> => {
    return CreateSMChannelFromStream(streamId);
  };

  const deleteSMChannels = (smchannelIds: number[]): Promise<DefaultAPIResponse | null> => {
    return DeleteSMChannels(smchannelIds);
  };

  const deleteSMChannel = (smchannelId: number): Promise<DefaultAPIResponse | null> => {
    return DeleteSMChannel(smchannelId);
  };

  const deleteAllSMChannelsFromParameters = (Parameters: QueryStringParameters): Promise<DefaultAPIResponse | null> => {
    return DeleteAllSMChannelsFromParameters(Parameters);
  };

  const addSMStreamToSMChannel = (request: SMStreamSMChannelRequest): Promise<DefaultAPIResponse | null> => {
    return AddSMStreamToSMChannel(request);
  };

  const removeSMStreamFromSMChannel = (request: SMStreamSMChannelRequest): Promise<DefaultAPIResponse | null> => {
    return RemoveSMStreamFromSMChannel(request);
  };

  const setSMStreamRanks = (requests: SMChannelRankRequest[]): Promise<DefaultAPIResponse | null> => {
    return SetSMStreamRanks(requests);
  };

  return { data, error, isError, isLoading, createSMChannelFromStream, deleteSMChannels, deleteSMChannel, deleteAllSMChannelsFromParameters, addSMStreamToSMChannel, removeSMStreamFromSMChannel, setSMStreamRanks, refreshSMChannels, setSMChannelsField };
};

export default useSMChannels;
