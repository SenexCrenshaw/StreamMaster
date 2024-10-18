import { GetSMChannelChannels } from '@lib/smAPI/SMChannelChannelLinks/SMChannelChannelLinksCommands';
import { GetSMChannelChannelsRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMChannelChannels = createAsyncThunk('cache/getGetSMChannelChannels', async (param: GetSMChannelChannelsRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    Logger.debug('Fetching GetSMChannelChannels');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetSMChannelChannels(param);
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetSMChannelChannels completed in ${duration.toFixed(2)}ms`);
    }

    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


