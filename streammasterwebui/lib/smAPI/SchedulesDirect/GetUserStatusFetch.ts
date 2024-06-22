import { GetUserStatus } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetUserStatus = createAsyncThunk('cache/getGetUserStatus', async (_: void, thunkAPI) => {
  try {
    const response = await GetUserStatus();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


