import { GetClientStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetClientStreamingStatistics = createAsyncThunk('cache/getGetClientStreamingStatistics', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetClientStreamingStatistics');
    const response = await GetClientStreamingStatistics();
    console.log('Fetched GetClientStreamingStatistics ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


