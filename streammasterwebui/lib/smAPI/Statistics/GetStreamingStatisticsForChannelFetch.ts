import { GetStreamingStatisticsForChannel } from '@lib/smAPI/Statistics/StatisticsCommands';
import { GetStreamingStatisticsForChannelRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamingStatisticsForChannel = createAsyncThunk('cache/getGetStreamingStatisticsForChannel', async (param: GetStreamingStatisticsForChannelRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    Logger.debug('Fetching GetStreamingStatisticsForChannel');
    const response = await GetStreamingStatisticsForChannel(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


