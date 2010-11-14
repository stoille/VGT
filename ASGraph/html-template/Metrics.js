//finds the correlation coefficient between two sets
function FindCorrelation(x,y)
{
	var sum_x = 0, sum_y = 0, sum_xy = 0, sum_xx = 0, sum_yy = 0;
	for(var i = 0; i < x.length; ++i){
		sum_x += x[i];
		sum_y += y[i];
		sum_xy += x[i]*y[i];
		sum_xx += x[i]*x[i];
		sum_yy += y[i]*y[i];
	}
	var n = x.length;
	return ( n * sum_xy - sum_x*sum_y/Math.sqrt((n*sum_xx-sum_xx)*(n*sum_yy-sym_yy)) );
}