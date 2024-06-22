import { GetHeadends } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { GetHeadendsRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetHeadends = createAsyncThunk('cache/getGetHeadends', async (param: GetHeadendsRequest, thunkAPI) => {
  try {
    const response = await GetHeadends(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


