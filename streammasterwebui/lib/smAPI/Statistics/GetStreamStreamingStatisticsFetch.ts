import { GetStreamStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamStreamingStatistics = createAsyncThunk('cache/getGetStreamStreamingStatistics', async (_: void, thunkAPI) => {
  try {
    const response = await GetStreamStreamingStatistics();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


