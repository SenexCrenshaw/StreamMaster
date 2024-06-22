import { GetStreamingStatisticsForChannel } from '@lib/smAPI/Statistics/StatisticsCommands';
import { GetStreamingStatisticsForChannelRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamingStatisticsForChannel = createAsyncThunk('cache/getGetStreamingStatisticsForChannel', async (param: GetStreamingStatisticsForChannelRequest, thunkAPI) => {
  try {
    const response = await GetStreamingStatisticsForChannel(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


