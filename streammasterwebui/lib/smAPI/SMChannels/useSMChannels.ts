import { FieldData, GetApiArgument, PagedResponse, QueryHookResult,,DefaultAPIResponse,QueryStringParameters,SMChannelRankRequest } from '@lib/apiDefs';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { AddSMStreamToSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CreateSMChannelFromStream } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteAllSMChannelsFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMChannels } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { RemoveSMStreamFromSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SetSMChannelLogo } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SetSMStreamRanks } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { fetchGetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsFetch';
import { clearSMChannels, updateSMChannels } from '@lib/smAPI/SMChannels/SMChannelsSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<> | undefined> {}

interface Result extends ExtendedQueryHookResult {
  addSMStreamToSMChannel: (SMChannelId: number, SMStreamId: string) => Promise<DefaultAPIResponse | null>;
  createSMChannelFromStream: (streamId: string) => Promise<DefaultAPIResponse | null>;
  deleteAllSMChannelsFromParameters: (Parameters: QueryStringParameters) => Promise<DefaultAPIResponse | null>;
  deleteSMChannel: (smChannelId: number) => Promise<DefaultAPIResponse | null>;
  deleteSMChannels: (smChannelIds: number[]) => Promise<DefaultAPIResponse | null>;
  removeSMStreamFromSMChannel: (SMChannelId: number, SMStreamId: string) => Promise<DefaultAPIResponse | null>;
  setSMChannelLogo: (SMChannelId: number, logo: string) => Promise<DefaultAPIResponse | null>;
  setSMStreamRanks: (requests: SMChannelRankRequest[]) => Promise<DefaultAPIResponse | null>;
  setSMChannelsField: (fieldData: FieldData) => void;
  refreshSMChannels: () => void;
}

const useSMChannels = (params?: GetApiArgument | undefined): Result => {
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

  const addSMStreamToSMChannel = (SMChannelId: number, SMStreamId: string): Promise<DefaultAPIResponse | null> => {
    return AddSMStreamToSMChannel(SMChannelId, SMStreamId);
  };

  const createSMChannelFromStream = (streamId: string): Promise<DefaultAPIResponse | null> => {
    return CreateSMChannelFromStream(streamId);
  };

  const deleteAllSMChannelsFromParameters = (Parameters: QueryStringParameters): Promise<DefaultAPIResponse | null> => {
    return DeleteAllSMChannelsFromParameters(Parameters);
  };

  const deleteSMChannel = (smChannelId: number): Promise<DefaultAPIResponse | null> => {
    return DeleteSMChannel(smChannelId);
  };

  const deleteSMChannels = (smChannelIds: number[]): Promise<DefaultAPIResponse | null> => {
    return DeleteSMChannels(smChannelIds);
  };

  const removeSMStreamFromSMChannel = (SMChannelId: number, SMStreamId: string): Promise<DefaultAPIResponse | null> => {
    return RemoveSMStreamFromSMChannel(SMChannelId, SMStreamId);
  };

  const setSMChannelLogo = (SMChannelId: number, logo: string): Promise<DefaultAPIResponse | null> => {
    return SetSMChannelLogo(SMChannelId, logo);
  };

  const setSMStreamRanks = (requests: SMChannelRankRequest[]): Promise<DefaultAPIResponse | null> => {
    return SetSMStreamRanks(requests);
  };

  return { data, error, isError, isLoading, addSMStreamToSMChannel, createSMChannelFromStream, deleteAllSMChannelsFromParameters, deleteSMChannel, deleteSMChannels, removeSMStreamFromSMChannel, setSMChannelLogo, setSMStreamRanks, refreshSMChannels, setSMChannelsField };
};

export default useSMChannels;
