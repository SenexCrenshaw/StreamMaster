import { GetService } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { GetServiceRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetService = createAsyncThunk('cache/getGetService', async (param: GetServiceRequest, thunkAPI) => {
  try {
    const response = await GetService(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


