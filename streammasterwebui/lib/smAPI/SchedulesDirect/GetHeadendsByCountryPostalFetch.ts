import { GetHeadendsByCountryPostal } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { GetHeadendsByCountryPostalRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetHeadendsByCountryPostal = createAsyncThunk('cache/getGetHeadendsByCountryPostal', async (param: GetHeadendsByCountryPostalRequest, thunkAPI) => {
  try {
    Logger.debug('Fetching GetHeadendsByCountryPostal');
    const response = await GetHeadendsByCountryPostal(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


