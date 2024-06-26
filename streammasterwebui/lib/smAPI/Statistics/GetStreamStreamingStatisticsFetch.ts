import { GetStreamStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetStreamStreamingStatistics = createAsyncThunk('cache/getGetStreamStreamingStatistics', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetStreamStreamingStatistics');
    const response = await GetStreamStreamingStatistics();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


