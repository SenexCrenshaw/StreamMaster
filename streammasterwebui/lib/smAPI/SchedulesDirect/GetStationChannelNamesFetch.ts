import { GetStationChannelNames } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStationChannelNames = createAsyncThunk('cache/getGetStationChannelNames', async (_: void, thunkAPI) => {
  try {
    const response = await GetStationChannelNames();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


