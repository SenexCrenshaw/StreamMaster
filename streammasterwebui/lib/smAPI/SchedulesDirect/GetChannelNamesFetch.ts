import { GetChannelNames } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelNames = createAsyncThunk('cache/getGetChannelNames', async (_: void, thunkAPI) => {
  try {
    const response = await GetChannelNames();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


